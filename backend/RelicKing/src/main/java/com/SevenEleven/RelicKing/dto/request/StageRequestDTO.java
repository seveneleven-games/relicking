package com.SevenEleven.RelicKing.dto.request;

import java.util.List;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.dto.request.model.SkillDTO;

import jakarta.validation.Valid;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Positive;
import jakarta.validation.constraints.PositiveOrZero;
import lombok.Data;

@Data
public class StageRequestDTO {

	@PositiveOrZero
	private Integer eliteKill;

	@PositiveOrZero
	private Integer normalKill;

	@Positive
	@NotNull
	@Max(Constant.MAX_STAGE)
	private Integer stage;

	@NotNull
	@Positive
	private Integer difficulty;

	@NotNull
	@Valid
	private List<SkillDTO> skillList;

}
