package com.SevenEleven.RelicKing.common.response;

import java.util.LinkedHashMap;

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
}
