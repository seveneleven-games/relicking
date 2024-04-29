package com.SevenEleven.RelicKing.dto.response;

import com.SevenEleven.RelicKing.dto.response.model.StageDifficultyDTO;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class LoginResponseDTO {

	@Builder.Default
	private String accessToken = "accessToken";

	@Builder.Default
	private String refreshToken = "refreshToken";

	private int memberId;

	private String nickname;

	private StageDifficultyDTO stageData;
}
