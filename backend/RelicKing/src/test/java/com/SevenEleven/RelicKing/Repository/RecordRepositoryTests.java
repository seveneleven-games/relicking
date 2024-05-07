package com.SevenEleven.RelicKing.Repository;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;

import lombok.extern.log4j.Log4j2;

import java.util.Random;

@SpringBootTest
@Log4j2
public class RecordRepositoryTests {

	@Autowired
	private RecordRepository recordRepository;

	@Autowired
	private MemberRepository memberRepository;


	@Test
	public void Test1() {
		Assertions.assertNotNull(recordRepository);
		log.info("--------------------------------------------------------------");
		log.info(recordRepository.getClass().getName());
		log.info("--------------------------------------------------------------");
	}

	@Test
	public void massInsertTest() {

		for (int i = 1; i <= 100; i++) {
			Member member = memberRepository.findByMemberId(i).orElseThrow();
			for (int k = 1; k <= 3; k++) {

				Random rand = new Random();
				if (rand.nextInt(3) % 3 == 0) {
					continue;
				}

				int stage = k;
				int difficulty = rand.nextInt(100);

				Record record = Record.builder()
						.member(member)
						.stage(stage)
						.difficulty(difficulty)
						.eliteKill(10)
						.normalKill(200)
						.classNo(member.getCurrentClassNo())
						.build();

				member.getMemberRelics().forEach(memberRelic -> {
					if (memberRelic.getSlot() > 0) {
						record.addRecordRelic(memberRelic.getRelicNo(), memberRelic.getLevel(), memberRelic.getSlot());
					}
				});

				int size = record.getRecordRelics().size();
				if (size < 6) {
					for (int j = size + 1; j <= 6; j++) {
						record.addRecordRelic(0, 0, j);
					}
				}

				for (int j = 1; j <= 6; j++) {
					int skillNo = rand.nextInt(Constant.THE_NUMBER_OF_SKILL);
					record.addRecordSkill(skillNo, skillNo > 0 ? rand.nextInt(9) + 1 : 0, j);
				}

				recordRepository.save(record);
			}
		}
	}

	// @Test
	// public void insertTest() {
	//
	// 	Member member = memberRepository.findById(1).orElseThrow();
	//
	// 	for (int i = 1; i <= 3; i++) {
	// 		int stage = i;
	// 		int difficulty = 10 - i;
	//
	// 		Record record = Record.builder()
	// 				.member(member)
	// 				.stage(stage)
	// 				.difficulty(difficulty)
	// 				.eliteKill(10)
	// 				.normalKill(200)
	// 				.classNo(1)
	// 				.build();
	//
	// 		for (int j = 1; j <= 6; j++) {
	// 			record.addRecordRelic(j, 10, j);
	// 		}
	//
	// 		for (int j = 1; j <= 6; j++) {
	// 			record.addRecordSkill(j, 10, j);
	// 		}
	//
	// 		recordRepository.save(record);
	// 	}
	// }

	@Transactional
	@Test
	public void readTest() {
		log.info("================================================================");
		log.info(recordRepository.findRankByMemberAndStage(1, 1));
	}

	@Test
	public void deleteTest() {
		int id = 1;
		recordRepository.deleteById(id);
	}

}
