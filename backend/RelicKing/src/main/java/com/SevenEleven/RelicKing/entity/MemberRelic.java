package com.SevenEleven.RelicKing.entity;

import com.SevenEleven.RelicKing.common.Constant;
import com.SevenEleven.RelicKing.dto.model.MemberRelicDTO;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Entity
@Getter
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@ToString
public class MemberRelic implements Comparable<MemberRelic> {
	@Id
	@Column(name = "member_relic_id")
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int memberRelicId;

	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "member_id", referencedColumnName = "member_id", nullable = false)
	private Member member;

	@Column(nullable = false, updatable = false)
	private int relicNo = 0;

	@Column(nullable = false)
	private int level = 1;

	@Column(nullable = false)
	private int exp = 0;

	@Column(nullable = false)
	private int slot = 0;

	public MemberRelic(Member member, int relicNo) {
		this.member = member;
		this.relicNo = relicNo;
	}

	public static MemberRelicDTO entityToDTO(MemberRelic memberRelic) {
		return MemberRelicDTO.builder()
			.relicNo(memberRelic.getRelicNo())
			.level(memberRelic.getLevel())
			.exp(memberRelic.getExp())
			.slot(memberRelic.getSlot())
			.build();
	}

	public void changeSlot(int slot) {
		this.slot = slot;
	}

	public void plusExp(int exp) {
		if (this.level != Constant.LEVEL_EXP_TABLE.size()){
			this.exp += exp;
			log.info("[유물 경험치 증가] (email){} (relicNo){} (plusExp){}", this.member.getEmail(), this.relicNo, exp);
			setLevel();
		}
	}

	private void setLevel() {
		if (this.level < Constant.LEVEL_EXP_TABLE.size()) {
			for (int i = 0; i < Constant.LEVEL_EXP_TABLE.size(); i++) {
				if (this.exp < Constant.LEVEL_EXP_TABLE.get(i)) {
					this.level = i + 1;
					log.info("[유물 레벨업] (email){} (relicNo){} (afterLevel){}", this.member.getEmail(), this.relicNo, this.level);
					break;
				}
			}
		}
	}

	@Override
	public int compareTo(MemberRelic memberRelic) {
		if (memberRelic.getRelicNo() > relicNo) {
			return 1;
		} else if (memberRelic.getRelicNo() < relicNo) {
			return -1;
		}
		return 0;
	}
}
