package com.SevenEleven.RelicKing.common.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;

@Configuration
@EnableWebSecurity
public class SecurityConfig {

	private final String[] whiteList = {
		"/api/members/login",
		"/api/members/temp-password",
		"/api/members/duplicate-email",
		"/api/members/duplicate-nickname",
		"/api/members/email-code",
		"/api/members/email-authenticate",
		"/api/members/signup",
		"/api/members/kakao-login",
	};

	@Bean
	public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {

		http
			.csrf(AbstractHttpConfigurer::disable)    // csrf disable
			.formLogin(AbstractHttpConfigurer::disable)    // form 로그인 방식 disable
			.httpBasic(AbstractHttpConfigurer::disable)    // http basic 인증 방식 disable

			// 경로별 인가 작업
			.authorizeHttpRequests(auth -> auth
				.requestMatchers(whiteList).permitAll()
				.anyRequest().authenticated())

			// 세션 사용하지 않음
			.sessionManagement(session ->
				session.sessionCreationPolicy(SessionCreationPolicy.STATELESS));

		return http.build();
	}

	@Bean
	public BCryptPasswordEncoder bCryptPasswordEncoder() {
		return new BCryptPasswordEncoder();
	}
}
