package com.SevenEleven.RelicKing.common.security;

public interface JWTProperties {
	Long ACCESS_TOKEN_EXPIRATION_TIME = 864000000L;    // 10일	FIXME : 시간 짧게 조정하고 reissue 로직 제대로 구현하기
	Long REFRESH_TOKEN_EXPIRATION_TIME = 864000000L;    // 10일
	String ACCESS_TOKEN_PREFIX = "Bearer ";
}
