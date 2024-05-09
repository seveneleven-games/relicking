package com.SevenEleven.RelicKing.controller;

import java.util.TimeZone;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.common.security.CustomUserDetails;
import com.SevenEleven.RelicKing.dto.request.SaveLockRequestDto;
import com.SevenEleven.RelicKing.dto.response.GetLockInfoResponseDto;
import com.SevenEleven.RelicKing.dto.response.SaveLockResponseDto;
import com.SevenEleven.RelicKing.service.LockService;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;

@Tag(name = "Lock", description = "방치 관련 API")
@RestController
@RequiredArgsConstructor
@RequestMapping("/api/lock")
public class LockController {

	private final LockService lockService;

	@Operation(
		summary = "방치 종료",
		description = "방치를 종료하고 통계를 업데이트하며 가챠를 획득합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "방치 종료 및 후처리 완료",
		content = @Content(schema = @Schema(implementation = SaveLockResponseDto.class))
	)
	@PostMapping("/end")
	public Response saveLock(@AuthenticationPrincipal CustomUserDetails customUserDetails, @RequestBody SaveLockRequestDto saveLockRequestDto) {
		SaveLockResponseDto saveLockResponseDto = lockService.saveLock(customUserDetails.getMember(), saveLockRequestDto);
		return new Response(HttpStatus.OK.value(), "방치가 종료되었습니다.", saveLockResponseDto);
	}

	@Operation(
		summary = "방치 정보 조회",
		description = "방치 정보를 조회합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "방치 정보 조회 성공",
		content = @Content(schema = @Schema(implementation = GetLockInfoResponseDto.class))
	)
	@GetMapping
	public Response getLockInfo(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		GetLockInfoResponseDto getLockInfoResponseDto = lockService.getLockInfo(customUserDetails.getMember());
		return new Response(HttpStatus.OK.value(), "방치 정보를 조회했습니다.", getLockInfoResponseDto);
	}
}
