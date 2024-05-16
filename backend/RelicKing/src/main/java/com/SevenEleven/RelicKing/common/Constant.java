package com.SevenEleven.RelicKing.common;

import java.util.Arrays;
import java.util.List;

public interface Constant {

	// in game
	int MAX_STAGE = 3;

	// gacha
	int INITIAL_GACHA = 100;    // 회원가입 시 가챠권 개수
	int SECONDS_PER_GACHA = 10;    // 가챠권 하나 얻기 위해 필요한 시간
	int LOCK_TIME_TO_CONTINUE = 3600;    // 1시간 이상 방치해야 스트릭 유지
	int[] BONUS_GACHA_PERCENTAGE = new int[] {0, 10, 20, 30, 40, 50, 60, 70};
	int EXP_GACHA = 1000;
	double[] GACHA_POSSIBILITY = {0, 0.6, 0.9, 0.98, 0.9999, 1};

	// jwt
	Long ACCESS_TOKEN_EXPIRATION_TIME = 86400000L;    // 1일
	Long REFRESH_TOKEN_EXPIRATION_TIME = 7776000000L;    // 90일
	String ACCESS_TOKEN_PREFIX = "Bearer ";

	// relic
	List<Integer> LEVEL_EXP_TABLE = Arrays.asList(20000, 40000, 80000, 160000, 320000, 640000, 1280000, 2560000, 5120000);
	int[] RELIC_INFO_TABLE = {4, 5, 5, 6, 1}; // 유물 등급 별 개수. 차례로 C, B, A, S, SSS
	int[] RARITY_WEIGHT = {2, 4, 7, 10, 10}; // 가챠 시 각 유물 등급별 경험치 가중치

	// email
	String AUTH_CODE_PREFIX = "AuthCode ";

	// skill
	int THE_NUMBER_OF_SKILL = 10;

	// app
	String APP_VERSION = "0.9.3";
}
