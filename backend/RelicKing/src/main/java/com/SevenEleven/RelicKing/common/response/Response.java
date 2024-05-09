package com.SevenEleven.RelicKing.common.response;

import java.io.IOException;
import java.util.LinkedHashMap;

import com.fasterxml.jackson.databind.ObjectMapper;

import jakarta.servlet.http.HttpServletResponse;

public class Response extends LinkedHashMap<String, Object> {

	public static final String STATUS = "status";
	public static final String MESSAGE = "message";
	public static final String DATA = "data";

	public Response(int status, String message, Object data) {
		setData(Response.STATUS, status);
		setData(Response.MESSAGE, message);
		setData(Response.DATA, data);
	}

	public void setData(String key, Object data) {
		put(key, data);
	}

	/**
	 * filter에서는 Response 객체를 return할 수 없으므로 직접 처리해주어야 한다.
	 */
	public static void setSuccessResponse(HttpServletResponse response, Response customResponse) throws IOException {

		ObjectMapper objectMapper = new ObjectMapper();

		response.setContentType("application/json");
		response.setCharacterEncoding("utf-8");

		String result = objectMapper.writeValueAsString(customResponse);

		response.getWriter().write(result);
	}
}
