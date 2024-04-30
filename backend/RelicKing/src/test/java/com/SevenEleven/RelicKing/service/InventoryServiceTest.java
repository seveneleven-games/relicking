package com.SevenEleven.RelicKing.service;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.dto.request.ChangeRelicRequestDTO;

import lombok.extern.log4j.Log4j2;

@SpringBootTest
@Log4j2
public class InventoryServiceTest {

	@Autowired
	private InventoryService inventoryService;

	@Test
	public void testChangeRelic() {
		ChangeRelicRequestDTO changeRelicRequestDTO = ChangeRelicRequestDTO.builder()
			.slot(1)
			.relicNo(7)
			.build();
		inventoryService.changeRelic(changeRelicRequestDTO);
	}
}
