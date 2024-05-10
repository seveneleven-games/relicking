package com.SevenEleven.RelicKing.Repository;

import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.EmptyStackException;
import java.util.List;
import java.util.Random;
import java.util.Stack;
import java.util.stream.IntStream;

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
	public void insertTest() {
		int totalRelicNo = Arrays.stream(Constant.RELIC_INFO_TABLE).sum();
		List<MemberRelic> newMemberRelics = new ArrayList<>();
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
					for (int k = 1; k < Constant.GACHA_POSSIBILITY.length; k++) {
						if (relicNoList[k - 1].isEmpty()) continue;
						if (Constant.GACHA_POSSIBILITY[k - 1] <= w && w < Constant.GACHA_POSSIBILITY[k]) {
							MemberRelic memberRelic = MemberRelic.builder()
									.member(member)
									.relicNo(relicNoList[k - 1].pop() + 100 * (k - 1))
									.build();
							newMemberRelics.add(memberRelic);
							break;
						}
					}
				}
			} catch (NoSuchAlgorithmException | EmptyStackException e) {
				throw new RuntimeException(e);
			}
		}
		memberRelicRepository.saveAll(newMemberRelics);
	}
}
