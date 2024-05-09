package com.SevenEleven.RelicKing.dto.request.model;

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
public class SkillDTO {

	@PositiveOrZero
	@Max(6)
	private int slot;

	@PositiveOrZero
	private int skillNo;

	@PositiveOrZero
	@Max(10)
	private int level;

}
