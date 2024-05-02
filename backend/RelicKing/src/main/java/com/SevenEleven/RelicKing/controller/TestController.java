package com.SevenEleven.RelicKing.controller;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.dto.response.InventoryResponseDTO;
import com.SevenEleven.RelicKing.dto.response.LoginResponseDTO;
import com.SevenEleven.RelicKing.service.TestService;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;

@Tag(name = "Test", description = "더미 사용자 정보 제공하는 테스트용 API")
@RestController
@RequiredArgsConstructor
@RequestMapping("/api/test")
public class TestController {

	private final TestService testService;

	@Operation(
		summary = "더미 사용자 반환",
		description = "더미 사용자 정보 반환"
	)
	@ApiResponse(
		responseCode = "200", description = "로그인 되었습니다.",
		content = @Content(schema = @Schema(implementation = LoginResponseDTO.class))
	)
	@GetMapping("/login")
	public Response testLogin() {
		LoginResponseDTO loginResponseDTO = testService.getLoginData(1);
		return new Response(HttpStatus.OK.value(), "로그인 되었습니다.", loginResponseDTO);
	}

	@Operation(
		summary = "더미 인벤토리 반환",
		description = "더미 사용자 인벤토리 정보 반환"
	)
	@ApiResponse(
		responseCode = "200", description = "인벤토리 정보 조회에 성공했습니다.",
		content = @Content(schema = @Schema(implementation = InventoryResponseDTO.class))
	)
	@GetMapping("/inventories")
	public Response getInventory() {
		InventoryResponseDTO inventoryResponseDTO = testService.getInventoryInfo(1);
		return new Response(HttpStatus.OK.value(), "인벤토리 정보 조회에 성공했습니다.", inventoryResponseDTO);
	}

}
