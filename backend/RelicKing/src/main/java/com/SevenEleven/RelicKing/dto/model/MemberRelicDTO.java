package com.SevenEleven.RelicKing.dto.model;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.PositiveOrZero;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class MemberRelicDTO {

	@PositiveOrZero
	private int relicNo;

	@PositiveOrZero
	private int level;

	@PositiveOrZero
	private int exp;

	@PositiveOrZero
	@Max(6)
	private int slot;
}
