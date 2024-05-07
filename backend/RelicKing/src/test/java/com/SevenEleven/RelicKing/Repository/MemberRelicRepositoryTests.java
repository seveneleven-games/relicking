package com.SevenEleven.RelicKing.Repository;

import java.util.Collections;
import java.util.List;
import java.util.Random;
import java.util.stream.IntStream;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.common.Constant;
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
		for (int i = 1; i <= 100; i++) {
			Member member = memberRepository.findByMemberId(i).orElseThrow();

			List<Integer> relicNoList = new java.util.ArrayList<>(
				IntStream.rangeClosed(1, Constant.THE_NUMBER_OF_RELICS).boxed().toList());
			Collections.shuffle(relicNoList);

			Random rand = new Random();

			for (int j = 1; j <= rand.nextInt(Constant.THE_NUMBER_OF_RELICS) + 1; j++ ) {
				MemberRelic memberRelic = MemberRelic.builder()
					.member(member)
					.relicNo(relicNoList.get(j - 1))
					.slot(j > 6 ? 0 : j)
					.build();
				memberRelic.plusExp(rand.nextInt(Constant.LEVEL_EXP_TABLE.get(9)));
				memberRelicRepository.save(memberRelic);
			}
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
