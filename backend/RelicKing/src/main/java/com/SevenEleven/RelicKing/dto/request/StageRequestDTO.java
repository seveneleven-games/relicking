package com.SevenEleven.RelicKing.dto.request;

import java.util.List;

import com.SevenEleven.RelicKing.dto.request.model.SkillDTO;

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
public class StageRequestDTO {

	@PositiveOrZero
	@NotNull
	private int eliteKill;

	@PositiveOrZero
	@NotNull
	private int normalKill;

	@PositiveOrZero
	@NotNull
	private int stage;

	@PositiveOrZero
	@NotNull
	private int difficulty;

	@NotNull
	private List<SkillDTO> skillList;

}
