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
    ;

    private final int status;
    private final String message;
}
