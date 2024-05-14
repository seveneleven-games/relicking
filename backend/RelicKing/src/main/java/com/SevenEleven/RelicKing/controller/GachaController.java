package com.SevenEleven.RelicKing.controller;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.common.security.CustomUserDetails;
import com.SevenEleven.RelicKing.dto.request.GachaRequestDTO;
import com.SevenEleven.RelicKing.service.GachaService;

import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Tag(name = "Gacha", description = "가챠 API")
@RestController
@RequiredArgsConstructor
@RequestMapping("/api/gacha")
public class GachaController {

	private final GachaService gachaService;

	@GetMapping()
	public Response getGachaInfo(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		return new Response(HttpStatus.OK.value(), "뽑기 재화 조회에 성공했습니다.", gachaService.getGachaInfo(customUserDetails.getMember()));
	}

	@PostMapping()
	public Response doGacha(@AuthenticationPrincipal CustomUserDetails customUserDetails, @RequestBody @Valid GachaRequestDTO gachaRequestDTO) {
		log.info("[가챠] email: {}, gachaNum: {}", customUserDetails.getMember().getEmail(), gachaRequestDTO.getGachaNum().getValue());
		return new Response(HttpStatus.OK.value(), "가챠 뽑기에 성공했습니다.", gachaService.doGacha(customUserDetails.getMember(), gachaRequestDTO));
	}

}
