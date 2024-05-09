package com.SevenEleven.RelicKing.dto.request;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.PositiveOrZero;
import lombok.Data;

@Data
public class RelicChangeRequestDTO {

	@NotNull
	@PositiveOrZero
	@Max(6)
	private Integer slot;

	@NotNull
	@PositiveOrZero
	private Integer relicNo;

}
