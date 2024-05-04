package com.SevenEleven.RelicKing.controller;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.common.security.CustomUserDetails;
import com.SevenEleven.RelicKing.service.GachaService;

import lombok.RequiredArgsConstructor;

@RestController
@RequiredArgsConstructor
@RequestMapping("/api/gacha")
public class GachaController {

	private final GachaService gachaService;

	@GetMapping()
	public Response getGachaInfo(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		return new Response(HttpStatus.OK.value(), "뽑기 재화 조회에 성공했습니다.", gachaService.getGachaInfo(customUserDetails.getMember()));
	}

}
