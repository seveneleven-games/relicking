package com.SevenEleven.RelicKing.service;

import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.time.Duration;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Optional;
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
import com.SevenEleven.RelicKing.dto.response.model.StageDifficultyDTO;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.entity.RefreshToken;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;
import com.SevenEleven.RelicKing.repository.RefreshTokenRepository;

import io.jsonwebtoken.ExpiredJwtException;
import io.jsonwebtoken.MalformedJwtException;
import io.jsonwebtoken.UnsupportedJwtException;
import io.jsonwebtoken.security.SecurityException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
@RequiredArgsConstructor
@Transactional
public class MemberService {

	private final MemberRepository memberRepository;
	private final BCryptPasswordEncoder bCryptPasswordEncoder;
	private final JWTUtil jwtUtil;
	private final RefreshTokenRepository refreshTokenRepository;
	private final RecordRepository recordRepository;
	private final EmailService emailService;
	private final RedisService redisService;

	@Value("${spring.mail.auth-code-expiration-millis}")
	private int authCodeExpirationMillis;

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
		Boolean isExist = refreshTokenRepository.existsByRefreshToken(refreshToken);
		if (!isExist) {
			ExceptionType exceptionType = ExceptionType.INVALID_JWT;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		}

		// JWT 생성
		String email = jwtUtil.getEmail(refreshToken);
		String newAccessToken = jwtUtil.createJwt("access", email, Constant.ACCESS_TOKEN_EXPIRATION_TIME);
		String newRefreshToken = jwtUtil.createJwt("refresh", email, Constant.REFRESH_TOKEN_EXPIRATION_TIME);    // Refresh Token Rotation

		// DB에 저장되어 있는 refresh token 모두 삭제
		refreshTokenRepository.deleteByEmail(email);

		// 새 Refresh 토큰 저장
		RefreshToken refreshTokenEntity = RefreshToken.builder()
			.email(email)
			.refreshToken(newRefreshToken)
			.expiration(new Date(System.currentTimeMillis() + Constant.REFRESH_TOKEN_EXPIRATION_TIME).toString())
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

		StageDifficultyDTO stageDifficultyDTO = getDifficulty(member);

		return LoginResponseDTO.builder()
			.accessToken(accessToken)
			.refreshToken(refreshToken)
			.memberId(member.getMemberId())
			.nickname(member.getNickname())
			.stageData(stageDifficultyDTO)
			.build();
	}

	@Transactional(readOnly = true)
	public StageDifficultyDTO getDifficulty(Member member) {

		List<Integer> stage = new ArrayList<>(Constant.MAX_STAGE);

		// FIXME : findByMemberAndStage 쿼리가 여러번 나가고 있다. member로 한 번에 조회할 수 있을 듯 하다.
		for (int i = 1; i <= Constant.MAX_STAGE; i++) {
			Optional<Record> record = recordRepository.findByMemberAndStage(member, i);
			if (record.isPresent()) {
				stage.add(record.get().getDifficulty());
			} else {
				stage.add(0);
			}
		}

		return StageDifficultyDTO.builder()
			.stage1(stage.get(0))
			.stage2(stage.get(1))
			.stage3(stage.get(2))
			.build();

	}

	@Transactional
	public void logout(Member member) {

		refreshTokenRepository.deleteByEmail(member.getEmail());
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
	public void updatePassword(Member member, String newPassword) {

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
		String text = "인증 코드 : " + authCode;	// TODO : 인증 코드 안내 메일 본문 예쁘게 꾸며보기
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
}
