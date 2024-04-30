package com.SevenEleven.RelicKing.common.security;

import java.io.IOException;
import java.util.Date;

import org.springframework.http.HttpStatus;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.AuthenticationException;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.dto.request.LoginRequestDto;
import com.SevenEleven.RelicKing.entity.RefreshToken;
import com.SevenEleven.RelicKing.repository.RefreshTokenRepository;
import com.fasterxml.jackson.databind.ObjectMapper;

import jakarta.servlet.FilterChain;
import jakarta.servlet.http.Cookie;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

public class CustomAuthenticationFilter extends UsernamePasswordAuthenticationFilter {

	private final AuthenticationManager authenticationManager;
	private final JWTUtil jwtUtil;
	private final RefreshTokenRepository refreshTokenRepository;

	public CustomAuthenticationFilter(AuthenticationManager authenticationManager, JWTUtil jwtUtil, RefreshTokenRepository refreshTokenRepository) {
		this.authenticationManager = authenticationManager;
		this.jwtUtil = jwtUtil;
		this.refreshTokenRepository = refreshTokenRepository;

		setFilterProcessesUrl("/api/members/login");
	}

	@Override
	public Authentication attemptAuthentication(HttpServletRequest request, HttpServletResponse response) throws AuthenticationException {

		// email, password 추출
		ObjectMapper om = new ObjectMapper();
		LoginRequestDto dto = null;
		try {
			dto = om.readValue(request.getInputStream(), LoginRequestDto.class);
		} catch (IOException e) {
			throw new CustomException(ExceptionType.IO_EXCEPTION);
		}
		String email = dto.getEmail();
		String password = dto.getPassword();

		// spring security에서는 검증을 위해 token에 담아야 함
		UsernamePasswordAuthenticationToken authToken = new UsernamePasswordAuthenticationToken(email, password, null);

		// AuthenticationManager로 전달
		return authenticationManager.authenticate(authToken);
	}

	@Override
	protected void successfulAuthentication(HttpServletRequest request, HttpServletResponse response, FilterChain chain, Authentication authentication) {

		// email 추출
		String email = authentication.getName();

		// 토큰 생성
		String accessToken = jwtUtil.createJwt("access", email, 600000L);
		String refreshToken = jwtUtil.createJwt("refresh", email, 86400000L);

		// Refresh 토큰 저장
		RefreshToken refreshTokenEntity = RefreshToken.builder()
			.email(email)
			.refreshToken(refreshToken)
			.expiration(new Date(System.currentTimeMillis() + 86400000L).toString())
			.build();
		refreshTokenRepository.save(refreshTokenEntity);

		// 응답 설정 TODO : body에 담는 식으로 바꾸기
		response.setHeader("accessToken", accessToken);
		response.addCookie(createCookie("refreshToken", refreshToken));
		response.setStatus(HttpStatus.OK.value());
	}

	@Override
	protected void unsuccessfulAuthentication(HttpServletRequest request, HttpServletResponse response, AuthenticationException failed) {

		response.setStatus(401);
	}

	private Cookie createCookie(String key, String value) {
		Cookie cookie = new Cookie(key, value);
		cookie.setMaxAge(24 * 60 * 60);
		cookie.setSecure(true);
		cookie.setHttpOnly(true);

		return cookie;
	}
}
