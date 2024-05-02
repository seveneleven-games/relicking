package com.SevenEleven.RelicKing.controller;

import com.SevenEleven.RelicKing.dto.request.ClassChangeRequestDTO;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.dto.request.RelicChangeRequestDTO;
import com.SevenEleven.RelicKing.dto.response.InventoryResponseDTO;
import com.SevenEleven.RelicKing.service.InventoryService;

import lombok.RequiredArgsConstructor;
import lombok.extern.log4j.Log4j2;

@RestController
@RequiredArgsConstructor
@RequestMapping("/api")
@Log4j2
public class InventoryController {

	private final InventoryService inventoryService;

	@GetMapping("/inventories")
	public Response getInventory() {
		InventoryResponseDTO inventoryResponseDTO = inventoryService.getInventoryInfo();
		return new Response(HttpStatus.OK.value(), "인벤토리 정보 조회에 성공했습니다.", inventoryResponseDTO);
	}

	@PostMapping("/classes")
	public Response changeClass(@RequestBody ClassChangeRequestDTO classChangeRequestDTO) {
		inventoryService.changeClass(classChangeRequestDTO.getClassNo());
		return new Response(HttpStatus.OK.value(), "클래스가 변경되었습니다.", true);
	}

	@PostMapping("/relics")
	public Response changeRelic(@RequestBody RelicChangeRequestDTO relicChangeRequestDTO) {
		inventoryService.changeRelic(relicChangeRequestDTO);
		return new Response(HttpStatus.OK.value(), "유물이 변경되었습니다.", true);
	}

}
