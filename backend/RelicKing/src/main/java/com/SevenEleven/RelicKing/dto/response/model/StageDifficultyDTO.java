package com.SevenEleven.RelicKing.dto.response.model;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class StageDifficultyDTO {

	@Builder.Default
	private int stage1 = 0;

	@Builder.Default
	private int stage2 = 0;

	@Builder.Default
	private int stage3 = 0;

}
