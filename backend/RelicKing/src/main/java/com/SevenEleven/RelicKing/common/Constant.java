package com.SevenEleven.RelicKing.common;

public interface Constant {

	// in game
	int MAX_STAGE = 3;

	// gacha
	int GACHA_PER_MINUTE = 1;	// 방치 1분 당 가챠권 지급 개수
	int LOCK_TIME_TO_CONTINUE = 3600;	// 1시간 이상 방치해야 스트릭 유지

	// jwt
	Long ACCESS_TOKEN_EXPIRATION_TIME = 864000000L;    // 10일	FIXME : 시간 짧게 조정하고 reissue 로직 제대로 구현하기
	Long REFRESH_TOKEN_EXPIRATION_TIME = 864000000L;    // 10일
	String ACCESS_TOKEN_PREFIX = "Bearer ";
}
