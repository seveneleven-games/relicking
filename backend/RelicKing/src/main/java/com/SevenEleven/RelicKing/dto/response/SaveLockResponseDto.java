package com.SevenEleven.RelicKing.dto.response;

import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class SaveLockResponseDto {

	private int earnedGacha;
	private int gachaAfterLock;
}
