package com.SevenEleven.RelicKing.dto.response;

import java.util.List;
import java.util.Map;

import com.SevenEleven.RelicKing.dto.model.RelicDTO;

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

	private Map<String, Integer> stageData;

	private int currentClassNo;

	private List<RelicDTO> relicList;

}
