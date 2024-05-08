package com.SevenEleven.RelicKing.common.response;

import java.io.IOException;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.fasterxml.jackson.databind.ObjectMapper;

import jakarta.servlet.http.HttpServletResponse;

public class ResponseFail extends Response {

	public ResponseFail(int status, String message) {
		super(status, message, false);
	}

	/**
	 * filter에서 발생하는 예외는 GlobalExceptionHandler로 처리할 수 없으므로 직접 처리해주어야 한다.
	 */
	public static void setErrorResponse(HttpServletResponse response, CustomException e) throws IOException {

		ObjectMapper objectMapper = new ObjectMapper();

		response.setStatus(e.getExceptionType().getStatus());
		response.setContentType("application/json");
		response.setCharacterEncoding("utf-8");

		ResponseFail responseFail = new ResponseFail(e.getExceptionType().getStatus(), e.getExceptionType().getMessage());

		response.getWriter().write(objectMapper.writeValueAsString(responseFail));
	}
}
