package com.SevenEleven.RelicKing.service;

import java.io.IOException;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.time.Duration;
import java.util.Arrays;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.common.security.JWTUtil;
import com.SevenEleven.RelicKing.dto.request.SignUpRequestDto;
import com.SevenEleven.RelicKing.dto.response.LoginResponseDTO;
import com.SevenEleven.RelicKing.dto.response.ReissueResponseDto;
import com.SevenEleven.RelicKing.dto.response.StageResponseDTO;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.entity.RefreshToken;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.SevenEleven.RelicKing.repository.RefreshTokenRepository;

import io.jsonwebtoken.ExpiredJwtException;
import io.jsonwebtoken.MalformedJwtException;
import io.jsonwebtoken.UnsupportedJwtException;
import io.jsonwebtoken.security.SecurityException;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
@RequiredArgsConstructor
public class MemberService {

	private final MemberRepository memberRepository;
	private final BCryptPasswordEncoder bCryptPasswordEncoder;
	private final JWTUtil jwtUtil;
	private final RefreshTokenRepository refreshTokenRepository;
	private final EmailService emailService;
	private final RedisService redisService;
	private final StageService stageService;

	@Value("${spring.mail.auth-code-expiration-millis}")
	private int authCodeExpirationMillis;

	@Transactional
	public void successfulAuthentication(HttpServletResponse response, String email) throws IOException {

		// 토큰 생성
		String accessToken = jwtUtil.createJwt("access", email, Constant.ACCESS_TOKEN_EXPIRATION_TIME);
		String refreshToken = jwtUtil.createJwt("refresh", email, Constant.REFRESH_TOKEN_EXPIRATION_TIME);

		// Refresh 토큰 저장
		RefreshToken refreshTokenEntity = RefreshToken.builder()
			.email(email)
			.refreshToken(refreshToken)
			.build();
		refreshTokenRepository.save(refreshTokenEntity);

		// 응답 설정
		LoginResponseDTO loginResponseDTO = getDataAfterLogin(email, accessToken, refreshToken);
		Response.setSuccessResponse(response, new Response(HttpStatus.OK.value(), "로그인 되었습니다.", loginResponseDTO));
	}

	@Transactional
	public void signup(SignUpRequestDto dto) {

		// 이미 존재하는 이메일일 경우 예외 처리
		if (memberRepository.existsByEmail(dto.getEmail())) {
			throw new CustomException(ExceptionType.EMAIL_ALREADY_EXISTS);
		}

		// 이미 존재하는 닉네임일 경우 예외 처리
		if (memberRepository.existsByNickname(dto.getNickname())) {
			throw new CustomException(ExceptionType.NICKNAME_ALREADY_EXISTS);
		}

		Member member = dto.toEntity(bCryptPasswordEncoder.encode(dto.getPassword()));
		member.changeGacha(10);

		memberRepository.save(member);
	}

	@Transactional
	public ReissueResponseDto reissue(String refreshToken) {

		// 정상적인 토큰인지 확인과 함께 토큰 만료 여부 확인
		try {
			jwtUtil.isExpired(refreshToken);
		} catch (SecurityException | MalformedJwtException e) {
			ExceptionType exceptionType = ExceptionType.INVALID_JWT;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		} catch (UnsupportedJwtException e) {
			ExceptionType exceptionType = ExceptionType.UNSUPPORTED_JWT;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		} catch (IllegalArgumentException e) {
			ExceptionType exceptionType = ExceptionType.JWT_CLAIMS_IS_EMPTY;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		} catch (ExpiredJwtException e) {
			ExceptionType exceptionType = ExceptionType.EXPIRED_JWT;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		}

		// 토큰이 refresh인지 확인 (발급시 페이로드에 명시)
		String category = jwtUtil.getCategory(refreshToken);

		if (!category.equals("refresh")) {
			ExceptionType exceptionType = ExceptionType.NOT_REFRESH_TOKEN;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		}

		// DB에 저장되어 있는지 확인
		boolean isExist = refreshTokenRepository.existsByRefreshToken(refreshToken);
		if (!isExist) {
			ExceptionType exceptionType = ExceptionType.INVALID_JWT;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		}

		// JWT 생성
		String email = jwtUtil.getEmail(refreshToken);
		String newAccessToken = jwtUtil.createJwt("access", email, Constant.ACCESS_TOKEN_EXPIRATION_TIME);
		String newRefreshToken = jwtUtil.createJwt("refresh", email, Constant.REFRESH_TOKEN_EXPIRATION_TIME);    // Refresh Token Rotation

		// 새 Refresh 토큰 저장
		RefreshToken refreshTokenEntity = RefreshToken.builder()
			.email(email)
			.refreshToken(newRefreshToken)
			.build();
		refreshTokenRepository.save(refreshTokenEntity);

		// response
		return ReissueResponseDto.builder()
			.accessToken(newAccessToken)
			.refreshToken(newRefreshToken)
			.build();
	}

	@Transactional(readOnly = true)
	public LoginResponseDTO getDataAfterLogin(String email, String accessToken, String refreshToken) {

		Member member = memberRepository.findByEmail(email);
		StageResponseDTO stageResponseDTO = stageService.getClassAndRelics(member);

		Map<String, Integer> stageDifficultyInfo = getDifficulty(member);

		return LoginResponseDTO.builder()
			.accessToken(accessToken)
			.refreshToken(refreshToken)
			.memberId(member.getMemberId())
			.nickname(member.getNickname())
			.stageData(stageDifficultyInfo)
			.currentClassNo(stageResponseDTO.getCurrentClassNo())
			.relicList(stageResponseDTO.getRelicList())
			.build();
	}

	@Transactional(readOnly = true)
	public Map<String, Integer> getDifficulty(Member member) {

		int[] stageInfo = new int[Constant.MAX_STAGE];
		Arrays.fill(stageInfo, 0);

		List<Record> recordList = member.getRecords().stream().toList();
		recordList.forEach(record -> {
			stageInfo[record.getStage() - 1] = record.getDifficulty();
		});

		Map<String, Integer> result = new LinkedHashMap<>();
		for (int i = 0; i < stageInfo.length; i++) {
			result.put("stage" + (i + 1), stageInfo[i]);
		}

		return result;

	}

	@Transactional
	public void logout(Member member) {

		refreshTokenRepository.deleteById(member.getEmail());
	}

	@Transactional
	public void updateNickname(Member member, String newNickname) {

		// 이미 존재하는 닉네임일 경우 예외 처리
		if (memberRepository.existsByNickname(newNickname)) {
			throw new CustomException(ExceptionType.NICKNAME_ALREADY_EXISTS);
		}

		member.updateNickname(newNickname);
		memberRepository.save(member);
	}

	@Transactional
	public void updatePassword(Member member, String oldPassword, String newPassword, String newPasswordRe) {

		// 기존 비밀번호가 불일치하는 경우
		if (!bCryptPasswordEncoder.matches(oldPassword, member.getPassword())) {
			throw new CustomException(ExceptionType.INCORRECT_PASSWORD);
		}

		// 새 비밀번호와 새 비밀번호 재입력 값이 다른 경우
		if (!newPassword.equals(newPasswordRe)) {
			throw new CustomException(ExceptionType.UNEQUAL_PASSWORDS);
		}

		String newEncryptedPassword = bCryptPasswordEncoder.encode(newPassword);
		member.updatePassword(newEncryptedPassword);
		memberRepository.save(member);
	}

	@Transactional(readOnly = true)
	public Response checkEmailForDuplicates(String email) {
		boolean isDuplicate = memberRepository.existsByEmail(email);

		if (isDuplicate) {
			return new Response(HttpStatus.CONFLICT.value(), "사용 불가능한 이메일입니다.", false);
		} else {
			return new Response(HttpStatus.OK.value(), "사용 가능한 이메일입니다.", true);
		}
	}

	@Transactional(readOnly = true)
	public Response checkNicknameForDuplicates(String nickname) {
		boolean isDuplicate = memberRepository.existsByNickname(nickname);

		if (isDuplicate) {
			return new Response(HttpStatus.CONFLICT.value(), "사용 불가능한 닉네임 입니다.", false);
		} else {
			return new Response(HttpStatus.OK.value(), "사용 가능한 닉네임 입니다.", true);
		}
	}

	@Transactional
	public void sendCodeToEmail(String email) {

		String title = "RelicKing에서 인증 코드를 알려드립니다.";
		String authCode = createCode();
		String text = "인증 코드 : " + authCode;    // TODO : 인증 코드 안내 메일 본문 예쁘게 꾸며보기
		emailService.sendEmail(email, title, text);

		// 이메일 인증 요청 시 인증 코드를 redis에 저장
		redisService.setValues(Constant.AUTH_CODE_PREFIX + email, authCode, Duration.ofMillis(authCodeExpirationMillis));
	}

	private String createCode() {
		int length = 6;
		try {
			Random random = SecureRandom.getInstanceStrong();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < length; i++) {
				stringBuilder.append(random.nextInt(10));
			}
			return stringBuilder.toString();
		} catch (NoSuchAlgorithmException e) {
			ExceptionType exceptionType = ExceptionType.NO_SUCH_ALGORITHM;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		}
	}

	@Transactional(readOnly = true)
	public void verifyEmail(String email, String code) {
		String redisAuthCode = redisService.getValues(Constant.AUTH_CODE_PREFIX + email);

		// 인증 코드가 만료된 경우
		if (!redisService.checkExistsValue(redisAuthCode)) {
			throw new CustomException(ExceptionType.EXPIRED_AUTH_CODE);
		}

		// 인증 코드가 불일치한 경우
		if (!redisAuthCode.equals(code)) {
			throw new CustomException(ExceptionType.WRONG_AUTH_CODE);
		}
	}

	@Transactional
	public void updateTempPassword(String email) {

		Member member = memberRepository.findByEmail(email);

		// 해당 이메일로 된 사용자가 존재하지 않는 경우
		if (member == null) {
			throw new CustomException(ExceptionType.MEMBER_NOT_FOUND);
		}

		// 임시 비밀번호 생성
		String tempPassword = createTempPassword();

		// 임시 비밀번호로 변경 (암호화하여 저장)
		String encryptedTempPassword = bCryptPasswordEncoder.encode(tempPassword);
		member.updatePassword(encryptedTempPassword);
		memberRepository.save(member);

		// 이메일로 임시 비밀번호 발송
		String title = "RelicKing에서 임시 비밀번호를 알려드립니다.";
		String text = "임시 비밀번호로 로그인 후 비밀번호를 변경해주세요. 임시 비밀번호 : " + tempPassword;    // TODO : 인증 코드 안내 메일 본문 예쁘게 꾸며보기
		emailService.sendEmail(email, title, text);
	}

	private String createTempPassword() {

		int length = 10;
		String CHAR_SET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		int CHAR_SET_LENGTH = CHAR_SET.length();

		try {
			Random random = SecureRandom.getInstanceStrong();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < length; i++) {
				int index = random.nextInt(CHAR_SET_LENGTH);
				stringBuilder.append(CHAR_SET.charAt(index));
			}
			return stringBuilder.toString();
		} catch (NoSuchAlgorithmException e) {
			ExceptionType exceptionType = ExceptionType.NO_SUCH_ALGORITHM;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		}
	}
}
