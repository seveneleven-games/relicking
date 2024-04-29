package com.SevenEleven.RelicKing.dto.model;

import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class MemberRelicDTO {

	@NotNull
	private int relicNo;

	@NotNull
	private int level;

	@NotNull
	private int exp;

	@NotNull
	private int slot;
}
