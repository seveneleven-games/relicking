package com.SevenEleven.RelicKing.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.common.security.CustomUserDetails;
import com.SevenEleven.RelicKing.dto.request.StageRequestDTO;
import com.SevenEleven.RelicKing.dto.response.GetLockInfoResponseDto;
import com.SevenEleven.RelicKing.dto.response.StageResponseDTO;
import com.SevenEleven.RelicKing.service.StageService;

@Tag(name = "Stage", description = "스테이지 관련 API")
@RestController
@RequiredArgsConstructor
@Log4j2
@RequestMapping("/api/stages")
public class StageController {

	private final StageService stageService;

	@Operation(
		summary = "스테이지 진입 전 사용자 정보 조회",
		description = "사용자의 현재 클래스 및 유물 정보를 조회합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "사용자 클래스 및 유물 정보 조회 성공",
		content = @Content(schema = @Schema(implementation = StageResponseDTO.class))
	)
	@GetMapping()
	public Response getInfoBeforeEnterStage(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		return new Response(HttpStatus.OK.value(),
			"스테이지에 진입하여 정보를 불러옵니다.",
			stageService.getClassAndRelics(customUserDetails.getMember()));
	}

	@Operation(
		summary = "게임 클리어",
		description = "게임을 클리어하여 유물 경험치, 레벨 및 랭킹 정보를 업데이트합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "유물 및 랭킹 업데이트 성공",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@PatchMapping()
	public Response clearStage(@AuthenticationPrincipal CustomUserDetails customUserDetails, @RequestBody StageRequestDTO stageRequestDTO) {
		stageService.patchRelicAndRecord(customUserDetails.getMember(), stageRequestDTO);
		return new Response(HttpStatus.OK.value(), "게임이 클리어 되었습니다.", true);
	}

}
