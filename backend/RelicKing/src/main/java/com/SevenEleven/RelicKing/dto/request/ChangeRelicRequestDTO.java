package com.SevenEleven.RelicKing.dto.request;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.PositiveOrZero;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class ChangeRelicRequestDTO {

	@NotNull
	@PositiveOrZero
	@Max(6)
	private int slot;

	@NotNull
	@PositiveOrZero
	private int relicNo;

}
