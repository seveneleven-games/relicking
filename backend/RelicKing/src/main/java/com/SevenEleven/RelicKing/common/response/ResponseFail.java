package com.SevenEleven.RelicKing.common.response;

public class ResponseFail extends Response {

	public ResponseFail(int status, String message) {
		super(status, message, false);
	}
}
