package com.SevenEleven.RelicKing.common.security;

import java.io.IOException;

import org.springframework.http.HttpStatus;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.filter.OncePerRequestFilter;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.fasterxml.jackson.databind.ObjectMapper;

import io.jsonwebtoken.ExpiredJwtException;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;

@RequiredArgsConstructor
public class JWTFilter extends OncePerRequestFilter {

	private final JWTUtil jwtUtil;
	private final MemberRepository memberRepository;

	private final ObjectMapper objectMapper = new ObjectMapper();

	@Override
	protected void doFilterInternal(HttpServletRequest request, HttpServletResponse response, FilterChain filterChain) throws ServletException, IOException {

		// 헤더에서 accessToken에 담긴 토큰을 꺼냄
		String accessTokenWithPrefix = request.getHeader("Authorization");

		// 토큰이 없거나 잘못된 형식이면 다음 필터로 넘김
		if (accessTokenWithPrefix == null || !accessTokenWithPrefix.startsWith(Constant.ACCESS_TOKEN_PREFIX)) {
			filterChain.doFilter(request, response);
			return;
		}

		// prefix 떼내기
		String accessToken = accessTokenWithPrefix.substring(Constant.ACCESS_TOKEN_PREFIX.length());

		// 토큰 만료 여부 확인, 만료시 다음 필터로 넘기지 않음
		try {
			jwtUtil.isExpired(accessToken); // 만료되었으면 예외가 발생한다.
		} catch (ExpiredJwtException e) {
			toJsonResponse(response, new Response(HttpStatus.UNAUTHORIZED.value(), "access token이 만료되었습니다.", false));
			return;
		}

		// 토큰이 access인지 확인 (발급시 페이로드에 명시), 다음 필터로 넘기지 않음
		String category = jwtUtil.getCategory(accessToken);

		if (!category.equals("access")) {
			toJsonResponse(response, new Response(HttpStatus.UNAUTHORIZED.value(), "유효하지 않은 access token입니다.", false));
			return;
		}

		String email = jwtUtil.getEmail(accessToken);    // accessToken에서 email 값 추출

		Member member = memberRepository.findByEmail(email);

		CustomUserDetails customUserDetails = new CustomUserDetails(member);

		Authentication authToken = new UsernamePasswordAuthenticationToken(customUserDetails, null, customUserDetails.getAuthorities());
		SecurityContextHolder.getContext().setAuthentication(authToken);

		filterChain.doFilter(request, response);
	}

	// TODO : toJSonResponse를 통해 에러 반환하는 코드들 throw를 통해 에러 반환하도록 변경하기
	private void toJsonResponse(HttpServletResponse response, Response customResponse) throws IOException {
		// content type
		response.setContentType("application/json");
		response.setCharacterEncoding("utf-8");

		String result = objectMapper.writeValueAsString(customResponse);

		response.getWriter().write(result);
	}
}
