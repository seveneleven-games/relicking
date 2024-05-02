package com.SevenEleven.RelicKing.controller;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.common.security.CustomUserDetails;
import com.SevenEleven.RelicKing.service.StageService;

@RestController
@RequiredArgsConstructor
@Log4j2
@RequestMapping("/api/stages")
public class StageController {

	private final StageService stageService;

	@GetMapping()
	public Response getInfoBeforeEnterStage(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		return new Response(HttpStatus.OK.value(),
			"스테이지에 진입하여 정보를 불러옵니다.",
			stageService.getClassAndRelics(customUserDetails.getMember()));
	}

	@PatchMapping()
	public Response clearStage(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		stageService.patchRecord(customUserDetails.getMember());
		return new Response(HttpStatus.OK.value(), "게임이 클리어 되었습니다.", true);
	}

}
