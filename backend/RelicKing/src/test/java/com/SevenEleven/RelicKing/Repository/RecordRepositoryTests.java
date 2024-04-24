package com.SevenEleven.RelicKing.Repository;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;

import lombok.extern.log4j.Log4j2;

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
	public void insertTest() {

		Member member = memberRepository.findById(1).orElseThrow();

		int stage = 2;
		int difficulty = 3;

		Record record = Record.builder()
			.member(member)
			.stage(stage)
			.difficulty(difficulty)
			.eliteKill(10)
			.normalKill(200)
			.classNo(1)
			.build();

		for (int i = 1; i <= 6; i++) {
			record.addRecordRelic(i, 10, i);
		}

		for (int i = 1; i <= 6; i++) {
			record.addRecordSkill(i, 10, i);
		}

		recordRepository.save(record);
	}

	@Test
	public void readTest() {
		Record record = recordRepository.findById(1).orElseThrow();

		log.info("--------------------------------------------------------------");
		log.info(record);
		log.info("--------------------------------------------------------------");
	}

	@Test
	public void deleteTest() {
		int id = 1;
		recordRepository.deleteById(id);
	}

}
