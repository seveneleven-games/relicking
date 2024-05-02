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

import lombok.RequiredArgsConstructor;

@RestController
@RequiredArgsConstructor
@RequestMapping("/api/lock")
public class LockController {

	private final LockService lockService;

	@PostMapping("/end")
	public Response saveLock(@AuthenticationPrincipal CustomUserDetails customUserDetails, @RequestBody SaveLockRequestDto saveLockRequestDto) {
		SaveLockResponseDto saveLockResponseDto = lockService.saveLock(customUserDetails.getMember(), saveLockRequestDto);
		return new Response(HttpStatus.OK.value(), "방치가 종료되었습니다.", saveLockResponseDto);
	}

	@GetMapping
	public Response getLockInfo(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		GetLockInfoResponseDto getLockInfoResponseDto = lockService.getLockInfo(customUserDetails.getMember());
		return new Response(HttpStatus.OK.value(), "방치 정보를 조회했습니다.", getLockInfoResponseDto);
	}
}
