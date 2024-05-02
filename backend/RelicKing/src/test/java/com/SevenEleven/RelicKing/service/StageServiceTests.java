package com.SevenEleven.RelicKing.service;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.extern.log4j.Log4j2;

@SpringBootTest
@Log4j2
public class StageServiceTests {

	@Autowired
	private StageService stageService;

	@Autowired
	private MemberRepository memberRepository;

	@Test
	public void testRead() {
		Member member = memberRepository.findByMemberId(1).orElseThrow();
		log.info("=================================================================");
		log.info(stageService.getClassAndRelics(member));
		log.info("=================================================================");
	}
}
