package com.SevenEleven.RelicKing.common.security;

import java.io.IOException;

import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.AuthenticationException;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.common.response.ResponseFail;
import com.SevenEleven.RelicKing.dto.request.LoginRequestDto;
import com.SevenEleven.RelicKing.service.MemberService;
import com.fasterxml.jackson.databind.ObjectMapper;

import jakarta.servlet.FilterChain;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.extern.slf4j.Slf4j;

@Slf4j
public class CustomAuthenticationFilter extends UsernamePasswordAuthenticationFilter {

	private final AuthenticationManager authenticationManager;
	private final MemberService memberService;

	public CustomAuthenticationFilter(AuthenticationManager authenticationManager, MemberService memberService) {
		this.authenticationManager = authenticationManager;
		this.memberService = memberService;

		setFilterProcessesUrl("/api/members/login");
	}

	@Override
	public Authentication attemptAuthentication(HttpServletRequest request, HttpServletResponse response) throws AuthenticationException {

		// email, password 추출
		ObjectMapper om = new ObjectMapper();
		LoginRequestDto dto;
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

		memberService.successfulAuthentication(response, authentication.getName());
	}

	@Override
	protected void unsuccessfulAuthentication(HttpServletRequest request, HttpServletResponse response, AuthenticationException failed) throws IOException {
		ExceptionType exceptionType = ExceptionType.AUTHENTICATION_FAILED;
		ResponseFail.setErrorResponse(response, new CustomException(exceptionType));
		log.info(exceptionType.getMessage());
	}

}
