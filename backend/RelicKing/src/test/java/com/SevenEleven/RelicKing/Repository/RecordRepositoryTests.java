package com.SevenEleven.RelicKing.Repository;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.entity.Member;
import com.SevenEleven.RelicKing.entity.Record;
import com.SevenEleven.RelicKing.entity.RecordRelic;
import com.SevenEleven.RelicKing.repository.MemberRepository;
import com.SevenEleven.RelicKing.repository.RecordRepository;

import lombok.extern.log4j.Log4j2;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

@SpringBootTest
@Log4j2
public class RecordRepositoryTests {

	@Autowired
	private RecordRepository recordRepository;

	@Autowired
	private MemberRepository memberRepository;

	@Test
	public void massInsertTest() {
		List<Record> recordList = new ArrayList<>();
		for (int i = 1; i <= memberRepository.count(); i++) {
			Member member = memberRepository.findByMemberId(i).orElseThrow();
			for (int k = 1; k <= Constant.MAX_STAGE; k++) {

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

				List<RecordRelic> recordRelics = record.getRecordRelics();
				int size = recordRelics.size();
				if (size < 6) {
					for (int j = size + 1; j <= 6; j++) {
						record.addRecordRelic(0, 0, j);
					}
				}

				recordRelics.sort(RecordRelic::compareTo);

				for (int j = 1; j <= 6; j++) {
					int skillNo = rand.nextInt(Constant.THE_NUMBER_OF_SKILL);
					record.addRecordSkill(skillNo, skillNo > 0 ? rand.nextInt(9) + 1 : 0, j);
				}
				recordList.add(record);
			}
		}
		recordRepository.saveAll(recordList);
	}
}
