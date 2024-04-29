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
    ;

    private final int status;
    private final String message;
}
