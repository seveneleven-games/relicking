package com.SevenEleven.RelicKing.service;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Optional;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
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
import com.SevenEleven.RelicKing.dto.response.model.StageDifficultyDTO;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.entity.RefreshToken;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;
import com.SevenEleven.RelicKing.repository.RefreshTokenRepository;

import io.jsonwebtoken.ExpiredJwtException;
import jakarta.servlet.http.Cookie;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
@Transactional
public class MemberService {

	private final MemberRepository memberRepository;
	private final BCryptPasswordEncoder bCryptPasswordEncoder;
	private final JWTUtil jwtUtil;
	private final RefreshTokenRepository refreshTokenRepository;
	private final RecordRepository recordRepository;

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

	public ResponseEntity<?> reissue(HttpServletRequest request, HttpServletResponse response) {
		// get refresh token
		String refreshToken = null;
		Cookie[] cookies = request.getCookies();
		for (Cookie cookie : cookies) {
			if (cookie.getName().equals("refreshToken")) {
				refreshToken = cookie.getValue();
			}
		}

		if (refreshToken == null) {
			return new ResponseEntity<>("refresh token expired", HttpStatus.BAD_REQUEST);
		}

		// expired check
		try {
			jwtUtil.isExpired(refreshToken);
		} catch (ExpiredJwtException e) {
			return new ResponseEntity<>("refresh token expired", HttpStatus.BAD_REQUEST);
		}

		// 토큰의 category가 refresh인지 확인
		String category = jwtUtil.getCategory(refreshToken);

		if (!category.equals("refresh")) {
			return new ResponseEntity<>("invalid refresh token", HttpStatus.BAD_REQUEST);
		}

		// DB에 저장되어 있는지 확인
		Boolean isExist = refreshTokenRepository.existsByRefreshToken(refreshToken);
		if (!isExist) {
			return new ResponseEntity<>("invalid refresh token", HttpStatus.BAD_REQUEST);
		}

		// JWT 생성
		String email = jwtUtil.getEmail(refreshToken);
		String newAccessToken = jwtUtil.createJwt("access", email, Constant.ACCESS_TOKEN_EXPIRATION_TIME);
		String newRefreshToken = jwtUtil.createJwt("refresh", email, Constant.REFRESH_TOKEN_EXPIRATION_TIME);    // Refresh Token Rotation

		// 기존 Refresh 토큰 삭제
		refreshTokenRepository.deleteByRefreshToken(refreshToken);

		// 새 Refresh 토큰 저장
		RefreshToken refreshTokenEntity = RefreshToken.builder()
			.email(email)
			.refreshToken(newRefreshToken)
			.expiration(new Date(System.currentTimeMillis() + Constant.REFRESH_TOKEN_EXPIRATION_TIME).toString())
			.build();
		refreshTokenRepository.save(refreshTokenEntity);

		// response
		response.setHeader("accessToken", Constant.ACCESS_TOKEN_PREFIX + newAccessToken);
		response.addCookie(createCookie("refreshToken", newRefreshToken));

		return new ResponseEntity<>(HttpStatus.OK);
	}

	// TODO : 없애기
	private Cookie createCookie(String key, String value) {
		Cookie cookie = new Cookie(key, value);
		cookie.setMaxAge(24 * 60 * 60);
		cookie.setSecure(true);
		cookie.setHttpOnly(true);

		return cookie;
	}

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

	public Response checkEmailForDuplicates(String email) {
		boolean isDuplicate = memberRepository.existsByEmail(email);

		if (isDuplicate) {
			return new Response(HttpStatus.CONFLICT.value(), "사용 불가능한 이메일입니다.", false);
		} else {
			return new Response(HttpStatus.OK.value(), "사용 가능한 이메일입니다.", true);
		}
	}

	public Response checkNicknameForDuplicates(String nickname) {
		boolean isDuplicate = memberRepository.existsByNickname(nickname);

		if (isDuplicate) {
			return new Response(HttpStatus.CONFLICT.value(), "사용 불가능한 닉네임 입니다.", false);
		} else {
			return new Response(HttpStatus.OK.value(), "사용 가능한 닉네임 입니다.", true);
		}
	}

}
