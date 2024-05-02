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
    ACCESS_TOKEN_EXPIRED(HttpStatus.UNAUTHORIZED.value(), "access token이 만료되었습니다."),
    INVALID_ACCESS_TOKEN(HttpStatus.UNAUTHORIZED.value(), "유효하지 않은 access token입니다."),
    ;

    private final int status;
    private final String message;
}
