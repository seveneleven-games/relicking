package com.SevenEleven.RelicKing.Repository;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.MemberRelic;
import com.SevenEleven.RelicKing.repository.MemberRelicRepository;
import com.SevenEleven.RelicKing.repository.MemberRepository;

import lombok.extern.log4j.Log4j2;

@SpringBootTest
@Log4j2
public class MemberRelicRepositoryTests {

	@Autowired
	private MemberRelicRepository memberRelicRepository;

	@Autowired
	private MemberRepository memberRepository;

	@Test
	public void test1() {
		Assertions.assertNotNull(memberRelicRepository);
		log.info("--------------------------------------------------------------");
		log.info(memberRelicRepository.getClass().getName());
		log.info("--------------------------------------------------------------");
	}

	@Test
	public void insertTest() {
		Member member = memberRepository.findById(1).orElseThrow();
		for (int i = 1; i <= 6; i++ ) {
			MemberRelic memberRelic = MemberRelic.builder()
				.member(member)
				.relicNo(i)
				.slot(i)
				.build();

			MemberRelic result = memberRelicRepository.save(memberRelic);

			log.info("--------------------------------------------------------------");
			log.info(result);
			log.info("--------------------------------------------------------------");
		}
	}

	@Test
	public void readTest() {
		MemberRelic memberRelic = memberRelicRepository.findById(1).orElseThrow();

		log.info("--------------------------------------------------------------");
		log.info(memberRelic);
		log.info("--------------------------------------------------------------");
	}
}
