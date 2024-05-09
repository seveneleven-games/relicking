package com.SevenEleven.RelicKing.Repository;

import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.EmptyStackException;
import java.util.Random;
import java.util.Stack;
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
		int totalRelicNo = Arrays.stream(Constant.RELIC_INFO_TABLE).sum();
		for (int i = 1; i <= memberRepository.count(); i++) {
			Member member = memberRepository.findByMemberId(i).orElseThrow();

			int raritySize = Constant.RELIC_INFO_TABLE.length;
			Stack<Integer>[] relicNoList = new Stack[raritySize];
			for (int j = 0; j < raritySize; j++) {
				ArrayList<Integer> relicNoArray = new ArrayList<>(
					IntStream.rangeClosed(1, Constant.RELIC_INFO_TABLE[j]).boxed().toList());
				Collections.shuffle(relicNoArray);
				Stack<Integer> stack = new Stack<>();
				relicNoArray.forEach(stack::push);
				relicNoList[j] = stack;
			}

			try {
				Random rand = SecureRandom.getInstanceStrong();
				for (int j = 0; j < totalRelicNo; j++) {
					double w = rand.nextDouble();
					MemberRelic memberRelic = null;
					if (w <= 0.6) {
						if (relicNoList[0].isEmpty()) {
							continue;
						}
						memberRelic = MemberRelic.builder()
							.member(member)
							.relicNo(relicNoList[0].pop())
							.build();
					} else if (w <= 0.9) {
						if (relicNoList[1].isEmpty()) {
							continue;
						}
						memberRelic = MemberRelic.builder()
							.member(member)
							.relicNo(relicNoList[1].pop() + 100)
							.build();
					} else if (w <= 0.98) {
						if (relicNoList[2].isEmpty()) {
							continue;
						}
						memberRelic = MemberRelic.builder()
							.member(member)
							.relicNo(relicNoList[2].pop() + 100 * 2)
							.build();
					} else if (w <= 0.9999) {
						if (relicNoList[3].isEmpty()) {
							continue;
						}
						memberRelic = MemberRelic.builder()
							.member(member)
							.relicNo(relicNoList[3].pop() + 100 * 3)
							.build();
					} else {
						if (relicNoList[4].isEmpty()) {
							continue;
						}
						memberRelic = MemberRelic.builder()
							.member(member)
							.relicNo(relicNoList[4].pop() + 100 * 4)
							.build();
					}
					memberRelicRepository.save(memberRelic);
				}
			} catch (NoSuchAlgorithmException | EmptyStackException e) {
				throw new RuntimeException(e);
			}

			// List<Integer> relicNoList = new java.util.ArrayList<>(
			// 	IntStream.rangeClosed(1, Constant.THE_NUMBER_OF_C).boxed().toList());
			// Collections.shuffle(relicNoList);
			//
			// Random rand = new Random();
			//
			// for (int j = 1; j <= rand.nextInt(Constant.THE_NUMBER_OF_C) + 1; j++ ) {
			// 	MemberRelic memberRelic = MemberRelic.builder()
			// 		.member(member)
			// 		.relicNo(relicNoList.get(j - 1))
			// 		.slot(j > 6 ? 0 : j)
			// 		.build();
			// 	memberRelic.plusExp(Constant.LEVEL_EXP_TABLE.get(rand.nextInt(10)) - 1);
			// 	memberRelicRepository.save(memberRelic);
			// }
		}
	}
}
