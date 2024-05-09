package com.SevenEleven.RelicKing.controller;

import com.SevenEleven.RelicKing.common.security.CustomUserDetails;
import com.SevenEleven.RelicKing.common.validation.ValidationSequence;
import com.SevenEleven.RelicKing.dto.request.ClassChangeRequestDTO;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.dto.request.RelicChangeRequestDTO;
import com.SevenEleven.RelicKing.dto.response.InventoryResponseDTO;
import com.SevenEleven.RelicKing.service.InventoryService;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

@Tag(name = "Inventory", description = "인벤토리 관련 API")
@RestController
@RequiredArgsConstructor
@RequestMapping("/api")
@Log4j2
public class InventoryController {

	private final InventoryService inventoryService;

	@Operation(
		summary = "인벤토리 정보 조회",
		description = "인벤토리 정보를 조회합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "인벤토리 정보 조회 성공",
		content = @Content(schema = @Schema(implementation = InventoryResponseDTO.class))
	)
	@GetMapping("/inventories") // Todo 유물 비어있을 때 번호 0짜리 새 유물 객체 넘기기
	public Response getInventory(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		return new Response(
			HttpStatus.OK.value(),
			"인벤토리 정보 조회에 성공했습니다.",
			inventoryService.getInventoryInfo(customUserDetails.getMember()));
	}

	@Operation(
		summary = "클래스 변경",
		description = "클래스를 변경합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "클래스 변경 성공",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@PostMapping("/classes")
	public Response changeClass(@AuthenticationPrincipal CustomUserDetails customUserDetails, @RequestBody @Validated(ValidationSequence.class) ClassChangeRequestDTO classChangeRequestDTO) {
		inventoryService.changeClass(customUserDetails.getMember(), classChangeRequestDTO.getClassNo());
		return new Response(HttpStatus.OK.value(), "클래스가 변경되었습니다.", true);
	}

	@Operation(
		summary = "유물 변경",
		description = "특정 슬롯의 유물을 변경합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "유물 변경 성공",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@PostMapping("/relics") // Todo 유물 비어있을 때 바꾸는 로직
	public Response changeRelic(@AuthenticationPrincipal CustomUserDetails customUserDetails, @RequestBody @Valid RelicChangeRequestDTO relicChangeRequestDTO) {
		inventoryService.changeRelic(customUserDetails.getMember(), relicChangeRequestDTO);
		return new Response(HttpStatus.OK.value(), "유물이 변경되었습니다.", true);
	}

}
