package com.SevenEleven.RelicKing.controller;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.dto.response.InventoryResponseDTO;
import com.SevenEleven.RelicKing.dto.response.LoginResponseDTO;
import com.SevenEleven.RelicKing.service.TestService;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

@RestController
@RequiredArgsConstructor
@Log4j2
@RequestMapping("/test")
public class TestController {

	private final TestService testService;

	@GetMapping("/login")
	public Response testLogin() {
		LoginResponseDTO loginResponseDTO = testService.getLoginData(1);
		return new Response(HttpStatus.OK.value(), "로그인 되었습니다.", loginResponseDTO);
	}

	@GetMapping("/inventories")
	public Response getInventory() {
		InventoryResponseDTO inventoryResponseDTO = testService.getInventoryInfo(1);
		return new Response(HttpStatus.OK.value(), "인벤토리 정보 조회에 성공했습니다.", inventoryResponseDTO);
	}

}
