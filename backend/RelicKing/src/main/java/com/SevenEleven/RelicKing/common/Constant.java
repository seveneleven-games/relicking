package com.SevenEleven.RelicKing.common;

import java.util.Arrays;
import java.util.List;

public interface Constant {

	// in game
	int MAX_STAGE = 3;

	// gacha
	int GACHA_PER_MINUTE = 1;    // 방치 1분 당 가챠권 지급 개수
	int LOCK_TIME_TO_CONTINUE = 3600;    // 1시간 이상 방치해야 스트릭 유지
	int[] BONUS_GACHA_PERCENTAGE = new int[] {0, 10, 20, 30, 40, 50, 60, 70};

	// jwt
	Long ACCESS_TOKEN_EXPIRATION_TIME = 86400000L;    // 1일
	Long REFRESH_TOKEN_EXPIRATION_TIME = 7776000000L;    // 90일
	String ACCESS_TOKEN_PREFIX = "Bearer ";

	// relic
	List<Integer> LEVEL_EXP_TABLE = Arrays.asList(2000000, 4000000, 8000000, 16000000, 32000000, 64000000, 128000000, 256000000, 512000000, 1024000000);
	int THE_NUMBER_OF_RELICS = 6;
	int EXP_GACHA = 1000;

	// email
	String AUTH_CODE_PREFIX = "AuthCode ";

	// skill
	int THE_NUMBER_OF_SKILL = 7;
}
