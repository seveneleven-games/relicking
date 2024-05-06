package com.SevenEleven.RelicKing.Repository;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.transaction.annotation.Transactional;

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

		for (int k = 1; k <= 3; k++) {
			for (int i = 1; i <= 100; i++) {
				Member member = memberRepository.findById(i).orElseThrow();

				Random rand = new Random();
				int stage = k;
				int difficulty = rand.nextInt(100);

				Record record = Record.builder()
						.member(member)
						.stage(stage)
						.difficulty(difficulty)
						.eliteKill(10)
						.normalKill(200)
						.classNo(1)
						.build();

				for (int j = 1; j <= 6; j++) {
					record.addRecordRelic(j, 10, j);
				}

				for (int j = 1; j <= 6; j++) {
					record.addRecordSkill(j, 10, j);
				}

				recordRepository.save(record);
			}
		}
	}

	@Test
	public void insertTest() {

		Member member = memberRepository.findById(1).orElseThrow();

		for (int i = 1; i <= 3; i++) {
			int stage = i;
			int difficulty = 10 - i;

			Record record = Record.builder()
					.member(member)
					.stage(stage)
					.difficulty(difficulty)
					.eliteKill(10)
					.normalKill(200)
					.classNo(1)
					.build();

			for (int j = 1; j <= 6; j++) {
				record.addRecordRelic(j, 10, j);
			}

			for (int j = 1; j <= 6; j++) {
				record.addRecordSkill(j, 10, j);
			}

			recordRepository.save(record);
		}
	}

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
