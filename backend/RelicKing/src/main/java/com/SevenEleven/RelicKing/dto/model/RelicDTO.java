package com.SevenEleven.RelicKing.dto.model;

import com.SevenEleven.RelicKing.entity.MemberRelic;

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
public class RelicDTO {

	@PositiveOrZero
	@Builder.Default
	private int relicNo = 0;

	@PositiveOrZero
	@Builder.Default
	private int level = 0;

	@PositiveOrZero
	@Max(6)
	private int slot;

	public static RelicDTO entityToDTO(MemberRelic memberRelic) {
		return RelicDTO.builder()
			.relicNo(memberRelic.getRelicNo())
			.level(memberRelic.getLevel())
			.slot(memberRelic.getSlot())
			.build();
	}

}
