package com.SevenEleven.RelicKing.common.exception;

import org.springframework.http.HttpStatus;

import lombok.Getter;
import lombok.RequiredArgsConstructor;

@Getter
@RequiredArgsConstructor
public enum ExceptionType {

    // test
    TEST_400(HttpStatus.BAD_REQUEST.value(), "400 에러 테스트입니다."),
    TEST_500(HttpStatus.INTERNAL_SERVER_ERROR.value(), "500 에러 테스트입니다."),

    // 5xx (server error)
    IO_EXCEPTION(HttpStatus.INTERNAL_SERVER_ERROR.value(), "요청 처리 중 오류가 발생했습니다. 나중에 다시 시도해주세요."),

    // member
    EMAIL_ALREADY_EXISTS(HttpStatus.CONFLICT.value(), "이미 가입된 이메일입니다."),
    NICKNAME_ALREADY_EXISTS(HttpStatus.CONFLICT.value(), "이미 존재하는 닉네임입니다."),
    MEMBER_NOT_FOUND(HttpStatus.NOT_FOUND.value(), "사용자를 찾을 수 없습니다."),
    AUTHENTICATION_FAILED(HttpStatus.UNAUTHORIZED.value(), "로그인에 실패하였습니다."),

    // JWT
    INVALID_JWT(HttpStatus.UNAUTHORIZED.value(), "유효하지 않은 JWT입니다."),
    UNSUPPORTED_JWT(HttpStatus.UNAUTHORIZED.value(), "지원되지 않는 JWT입니다."),
    JWT_CLAIMS_IS_EMPTY(HttpStatus.UNAUTHORIZED.value(), "잘못된 JWT입니다."),
    EXPIRED_JWT(HttpStatus.UNAUTHORIZED.value(), "JWT가 만료되었습니다."),
    NOT_ACCESS_TOKEN(HttpStatus.UNAUTHORIZED.value(), "access token이 아닙니다."),
    NOT_REFRESH_TOKEN(HttpStatus.UNAUTHORIZED.value(), "refresh token이 아닙니다."),
    ;

    private final int status;
    private final String message;
}
