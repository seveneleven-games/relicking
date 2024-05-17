package com.SevenEleven.RelicKing.common.exception;

import org.springframework.security.core.AuthenticationException;

public class OutdatedVersionException extends AuthenticationException {
	public OutdatedVersionException(String msg) {
		super(msg);
	}
}
