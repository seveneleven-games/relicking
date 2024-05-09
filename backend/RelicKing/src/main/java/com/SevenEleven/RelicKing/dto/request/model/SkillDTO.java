package com.SevenEleven.RelicKing.dto.request.model;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.PositiveOrZero;
import lombok.Data;

@Data
public class SkillDTO {

	@PositiveOrZero
	@Max(6)
	private Integer slot;

	@PositiveOrZero
	private Integer skillNo;

	@PositiveOrZero
	@Max(9)
	private Integer level;

}
