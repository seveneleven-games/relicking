package com.SevenEleven.RelicKing.common.security;

import java.io.IOException;
import java.util.Date;

import org.springframework.http.HttpStatus;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.AuthenticationException;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.dto.request.LoginRequestDto;
import com.SevenEleven.RelicKing.dto.response.LoginResponseDTO;
import com.SevenEleven.RelicKing.entity.RefreshToken;
import com.SevenEleven.RelicKing.repository.RefreshTokenRepository;
import com.SevenEleven.RelicKing.service.MemberService;
import com.fasterxml.jackson.databind.ObjectMapper;

import jakarta.servlet.FilterChain;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.extern.slf4j.Slf4j;

@Slf4j
public class CustomAuthenticationFilter extends UsernamePasswordAuthenticationFilter {

	private final AuthenticationManager authenticationManager;
	private final JWTUtil jwtUtil;
	private final RefreshTokenRepository refreshTokenRepository;
	private final MemberService memberService;

	private final ObjectMapper objectMapper = new ObjectMapper();

	public CustomAuthenticationFilter(AuthenticationManager authenticationManager, JWTUtil jwtUtil, RefreshTokenRepository refreshTokenRepository, MemberService memberService) {
		this.authenticationManager = authenticationManager;
		this.jwtUtil = jwtUtil;
		this.refreshTokenRepository = refreshTokenRepository;
		this.memberService = memberService;

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
	protected void successfulAuthentication(HttpServletRequest request, HttpServletResponse response, FilterChain chain, Authentication authentication) throws IOException {

		// TODO : 이 많은 로직을 여기서 하는 게 맞나 싶다. Service로 이주시켜야 하나? 그러면 Repository 주입도 줄일 수 있을 것 같다.

		// email 추출
		String email = authentication.getName();

		// 토큰 생성
		String accessToken = jwtUtil.createJwt("access", email, Constant.ACCESS_TOKEN_EXPIRATION_TIME);
		String refreshToken = jwtUtil.createJwt("refresh", email, Constant.REFRESH_TOKEN_EXPIRATION_TIME);

		// Refresh 토큰 저장
		RefreshToken refreshTokenEntity = RefreshToken.builder()
			.email(email)
			.refreshToken(refreshToken)
			.expiration(new Date(System.currentTimeMillis() + Constant.REFRESH_TOKEN_EXPIRATION_TIME).toString())
			.build();
		refreshTokenRepository.save(refreshTokenEntity);

		// 응답 설정
		LoginResponseDTO loginResponseDTO = memberService.getDataAfterLogin(email, accessToken, refreshToken);
		toJsonResponse(response, new Response(HttpStatus.OK.value(), "로그인 되었습니다.", loginResponseDTO));
	}

	@Override
	protected void unsuccessfulAuthentication(HttpServletRequest request, HttpServletResponse response, AuthenticationException failed) throws IOException {
		log.info("로그인에 실패하였습니다.");
		toJsonResponse(response, new Response(HttpStatus.UNAUTHORIZED.value(), "로그인에 실패하였습니다.", false));
	}

	private void toJsonResponse(HttpServletResponse response, Response customResponse) throws IOException {
		// content type
		response.setContentType("application/json");
		response.setCharacterEncoding("utf-8");

		String result = objectMapper.writeValueAsString(customResponse);

		response.getWriter().write(result);
	}
}
