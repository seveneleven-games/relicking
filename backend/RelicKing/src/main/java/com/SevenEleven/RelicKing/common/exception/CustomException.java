package com.SevenEleven.RelicKing.common.exception;

import lombok.Getter;

@Getter
public class CustomException extends RuntimeException{

    private final ExceptionType exceptionType;

    public CustomException(ExceptionType exceptionType) {
        super(exceptionType.getMessage());
        this.exceptionType = exceptionType;
    }
}
