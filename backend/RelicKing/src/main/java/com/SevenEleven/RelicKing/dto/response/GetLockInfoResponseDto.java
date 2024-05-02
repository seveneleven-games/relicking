package com.SevenEleven.RelicKing.dto.response;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class GetLockInfoResponseDto {

	private int totalLockTime;
	private int continuousLockDate;
	private int todayLockTime;
}
