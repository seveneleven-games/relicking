package com.SevenEleven.RelicKing.common.security;

import java.io.IOException;

import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.filter.OncePerRequestFilter;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.SevenEleven.RelicKing.common.response.ResponseFail;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import io.jsonwebtoken.ExpiredJwtException;
import io.jsonwebtoken.MalformedJwtException;
import io.jsonwebtoken.UnsupportedJwtException;
import io.jsonwebtoken.security.SecurityException;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@RequiredArgsConstructor
public class JWTFilter extends OncePerRequestFilter {

	private final JWTUtil jwtUtil;
	private final MemberRepository memberRepository;

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

		// 정상적인 토큰인지 확인과 함께 토큰 만료 여부 확인
		// 만료시 다음 필터로 넘기지 않음
		try {
			jwtUtil.isExpired(accessToken); // 만료되었으면 예외가 발생한다.
		} catch (SecurityException | MalformedJwtException e) {
			ExceptionType exceptionType = ExceptionType.INVALID_JWT;
			ResponseFail.setErrorResponse(response, new CustomException(exceptionType));
			log.info(exceptionType.getMessage());
			return;
		} catch (UnsupportedJwtException e) {
			ExceptionType exceptionType = ExceptionType.UNSUPPORTED_JWT;
			ResponseFail.setErrorResponse(response, new CustomException(exceptionType));
			log.info(exceptionType.getMessage());
			return;
		} catch (IllegalArgumentException e) {
			ExceptionType exceptionType = ExceptionType.JWT_CLAIMS_IS_EMPTY;
			ResponseFail.setErrorResponse(response, new CustomException(exceptionType));
			log.info(exceptionType.getMessage());
			return;
		} catch (ExpiredJwtException e) {
			ExceptionType exceptionType = ExceptionType.EXPIRED_JWT;
			ResponseFail.setErrorResponse(response, new CustomException(exceptionType));
			log.info(exceptionType.getMessage());
			return;
		}

		// 토큰이 access인지 확인 (발급시 페이로드에 명시), 다음 필터로 넘기지 않음
		String category = jwtUtil.getCategory(accessToken);

		if (!category.equals("access")) {
			ExceptionType exceptionType = ExceptionType.NOT_ACCESS_TOKEN;
			ResponseFail.setErrorResponse(response, new CustomException(exceptionType));
			log.info(exceptionType.getMessage());
			return;
		}

		String email = jwtUtil.getEmail(accessToken);    // accessToken에서 email 값 추출

		Member member = memberRepository.findByEmail(email);

		CustomUserDetails customUserDetails = new CustomUserDetails(member);

		Authentication authToken = new UsernamePasswordAuthenticationToken(customUserDetails, null, customUserDetails.getAuthorities());
		SecurityContextHolder.getContext().setAuthentication(authToken);

		filterChain.doFilter(request, response);
	}
}
