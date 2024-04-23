package com.SevenEleven.RelicKing.Repository;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.entity.RecordSkill;
import com.SevenEleven.RelicKing.repository.RecordRepository;
import com.SevenEleven.RelicKing.repository.RecordSkillRepository;

import lombok.extern.log4j.Log4j2;

@SpringBootTest
@Log4j2
public class RecordSkillRepositoryTests {

	@Autowired
	private RecordSkillRepository recordSkillRepository;

	@Autowired
	private RecordRepository recordRepository;

	@Test
	public void test1() {
		Assertions.assertNotNull(recordSkillRepository);

		log.info("--------------------------------------------------------");
		log.info(recordSkillRepository.getClass().getName());
		log.info("--------------------------------------------------------");
	}

	@Test
	public void insertTest() {
		Record record = recordRepository.findById(1).orElseThrow();

		for (int i = 1; i <= 6; i++){
			RecordSkill recordSkill = RecordSkill.builder()
				.record(record)
				.skillNo(i)
				.level(10)
				.slot(i)
				.build();

			recordSkillRepository.save(recordSkill);
		}
	}

	@Test
	public void readTest() {
		RecordSkill recordSkill = recordSkillRepository.findById(1).orElseThrow();

		log.info("--------------------------------------------------------------");
		log.info(recordSkill);
		log.info("--------------------------------------------------------------");
	}

}
